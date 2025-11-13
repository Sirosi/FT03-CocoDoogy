const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class RejectFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;
        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) => {
            this.validateBeforeReject(docs.userPrivateData, fuid);

            transaction.update(refs.userPrivateRef, {
                [`friendReceivedList.${fuid}`]: admin.firestore.FieldValue.delete()
            });
            transaction.update(refs.friendsPrivateRef, {
                [`friendSentList.${uid}`]: admin.firestore.FieldValue.delete()
            });

            return "친구 요청이 성공적으로 거절되었습니다.";
        });
    }
    validateBeforeReject(userData, friendsUid) {
        if (!userData.friendReceivedList?.[friendsUid]) {
            throw new Error("받은 친구 요청을 찾을 수 없습니다.");
        }
    }
}

exports.rejectFriendsRequest = onCall(fnConfig, async (request) => {
    const rejecter = new RejectFriendsRequest();
    return rejecter.Operation(request);
});
