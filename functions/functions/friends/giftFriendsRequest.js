const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class GiftFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;

        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) => {
            this.validateBeforePresent(docs.userPrivateData, fuid);

            const currentTime = Date.now();
            const giftData = {
                giftId: admin.firestore().collection("_").doc().id,
                giftType: "bonusTicket",
                giftCount: 1,
                isClaimed: false,
                sentAt: currentTime,
                fromNickname: docs.userPublicProfile.nickName
            };

            transaction.update(refs.userPrivateRef, {
                [`friendsList.${fuid}.giftTime`]: currentTime
            });

            const friendGiftList = docs.friendsPrivateData.giftList || [];
            friendGiftList.push(giftData);

            transaction.update(refs.friendsPrivateRef, {
                giftList: friendGiftList,
            });

            return "선물 보내기가 성공적으로 완료되었습니다.";
        }, true);
    }

    validateBeforePresent(userData, friendsUid) {
        const lastPresentTime = userData.friendsList[friendsUid].giftTime || 0;
        const now = Date.now();
        const cooldownTime = 24 * 60 * 60 * 1000;

        if (now - lastPresentTime < cooldownTime) {
            const remainingTime = Math.ceil((cooldownTime - (now - lastPresentTime)) / (1000 * 60 * 60));
            throw new Error(`선물은 24시간에 한 번만 보낼 수 있습니다. ${remainingTime}시간 후에 다시 시도해주세요.`);
        }
    }
}

exports.giftFriendsRequest = onCall(fnConfig, async (request) => {
    const presenter = new GiftFriendsRequest();
    return presenter.Operation(request);
});

