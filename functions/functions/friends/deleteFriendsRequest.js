const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class DeleteFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;
        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) => {
            this.validateBeforeDelete(docs.userPrivateData, fuid);

            transaction.update(refs.userPrivateRef, {
                [`friendsList.${fuid}`]: admin.firestore.FieldValue.delete()
            });
            transaction.update(refs.friendsPrivateRef, {
                [`friendsList.${uid}`]: admin.firestore.FieldValue.delete()
            });

            return "친구가 성공적으로 삭제되었습니다.";
        });
    }

    validateBeforeDelete(userData, friendsUid) {
        if (!userData.friendsList?.[friendsUid]) {
            throw new Error("친구 목록에서 해당 사용자를 찾을 수 없습니다.");
        }
    }
}

exports.deleteFriendsRequest = onCall(fnConfig, async (request) => {
    const deleter = new DeleteFriendsRequest();
    return deleter.Operation(request);
});
