const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class ReceiveFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;

        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) => {
            this.validateBeforeReceive(docs.userPrivateData, fuid);

            transaction.update(refs.userPrivateRef, {
                [`friendReceivedList.${fuid}`]: admin.firestore.FieldValue.delete(),
                [`friendsList.${fuid}.nickName`]: docs.friendsPublicProfile.nickName,
                [`friendsList.${fuid}.giftTime`]: null,
            });
            transaction.update(refs.friendsPrivateRef, {
                [`friendSentList.${uid}`]: admin.firestore.FieldValue.delete(),
                [`friendsList.${uid}.nickName`]: docs.userPublicProfile.nickName,
                [`friendsList.${uid}.giftTime`]: null,
            });

            return "친구 요청이 성공적으로 수락되었습니다.";
        }, true);
    }

    validateBeforeReceive(userData, friendsUid) {
        if (!userData.friendReceivedList?.[friendsUid]) {
            throw new Error("받은 친구 요청을 찾을 수 없습니다.");
        }
    }
}

exports.receiveFriendsRequest = onCall(fnConfig, async (request) => {
    const receiver = new ReceiveFriendsRequest();
    return receiver.Operation(request);
});

