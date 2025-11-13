const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class CancelFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;

        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) => {
            this.validateBeforeCancel(docs.userPrivateData, fuid);

            transaction.update(refs.userPrivateRef, {
                [`friendSentList.${fuid}`]: admin.firestore.FieldValue.delete()
            });
            transaction.update(refs.friendsPrivateRef, {
                [`friendReceivedList.${uid}`]: admin.firestore.FieldValue.delete()
            });

            return "친구 요청이 성공적으로 취소되었습니다.";
        });
    }
    validateBeforeCancel(userData, friendsUid) {
        if (!userData.friendSentList?.[friendsUid]) {
            throw new Error("보낸 친구 요청을 찾을 수 없습니다.");
        }
    }
}

exports.cancelFriendsRequest = onCall(fnConfig, async (request) => {
    const canceler = new CancelFriendsRequest();
    return canceler.Operation(request);
});
