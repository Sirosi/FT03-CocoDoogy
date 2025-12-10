const { onCall } = require("firebase-functions/v2/https");
const { fnConfig } = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class GiftAllFriendsRequest extends Request {
    async Operation(request) {
        if (!request.auth || !request.auth.uid) {
            return { message: "로그인 필요", sentCount: 0, skipped: [] };
        }

        const uid = request.auth.uid;

        // sender 정보 가져오기
        const refs = this.getReferences(uid, uid);
        let docs;
        try {
            docs = await this.getDocuments(null, refs, true);
        } catch (err) {
            console.error("사용자 문서 조회 실패:", err);
            return { message: "사용자 문서 조회 실패", sentCount: 0, skipped: [] };
        }

        const senderPrivate = docs.userPrivateData;
        const senderPublic = docs.userPublicProfile;

        if (!senderPublic || !senderPublic.nickName) {
            return { message: "사용자 프로필 없음", sentCount: 0, skipped: [] };
        }

        const friendsList = senderPrivate.friendsList || {};
        const friendUids = Object.keys(friendsList);

        if (friendUids.length === 0) {
            return { message: "등록된 친구 없음", sentCount: 0, skipped: [] };
        }

        const currentTime = Date.now();
        const cooldownTime = 24 * 60 * 60 * 1000;

        let skipped = [];
        let sentCount = 0;

        for (const fuid of friendUids) {
            const lastGiftTime = friendsList[fuid]?.giftTime || 0;
            if (currentTime - lastGiftTime < cooldownTime) {
                skipped.push(fuid);
                continue;
            }

            try {
                await admin.firestore().runTransaction(async (transaction) => {
                    const friendPrivateRef = admin.firestore()
                        .collection("users").doc(fuid)
                        .collection("private").doc("data");

                    const friendPrivateDoc = await transaction.get(friendPrivateRef);
                    if (!friendPrivateDoc.exists) throw new Error(`Friend UID ${fuid} 문서 없음`);

                    const friendData = friendPrivateDoc.data() || {};
                    const giftList = friendData.giftList || [];

                    giftList.push({
                        giftId: admin.firestore().collection("_").doc().id,
                        giftType: "bonusTicket",
                        giftCount: 1,
                        isClaimed: false,
                        sentAt: currentTime,
                        fromNickname: senderPublic.nickName
                    });

                    transaction.update(friendPrivateRef, { giftList });
                    transaction.update(refs.userPrivateRef, {
                        [`friendsList.${fuid}.giftTime`]: currentTime
                    });
                });

                sentCount++;
            } catch (err) {
                console.error(`Friend UID ${fuid} 처리 실패:`, err);
                skipped.push(fuid);
            }
        }
        return `${sentCount}`;
    }
}

exports.giftAllFriendsRequest = onCall(fnConfig, async (request) => {
    const presenter = new GiftAllFriendsRequest();
    return presenter.Operation(request);
});
