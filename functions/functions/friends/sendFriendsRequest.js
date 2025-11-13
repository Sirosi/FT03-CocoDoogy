const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const Request = require("../request/Request");

class SendFriendsRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const fuid = request.data.friendsUid;

        return this.executeTransaction(uid, fuid, async(transaction, refs, docs) =>{
            this.validateBeforeSend(docs.userPrivateData, docs.friendsPrivateData, uid, fuid);

            transaction.update(refs.userPrivateRef, {
                [`friendSentList.${fuid}`]: docs.friendsPublicProfile.nickName
            });
            transaction.update(refs.friendsPrivateRef, {
                [`friendReceivedList.${uid}`]: docs.userPublicProfile.nickName
            });

            return "친구 요청이 성공적으로 전송되었습니다.";
        }, true);
    }

    validateBeforeSend(userData, friendsData, uid, fuid) {
        if (userData.friendsList?.[fuid]) {
            throw new Error("이미 친구인 사용자에게는 친구 요청을 보낼 수 없습니다.");
        }
        if (userData.friendSentList?.[fuid]) {
            throw new Error("이미 친구 요청을 보낸 사용자입니다.");
        }
        if (userData.friendReceivedList?.[fuid]) {
            throw new Error("상대방이 이미 당신에게 친구 요청을 보냈습니다.");
        }
        if (fuid === uid) {
            throw new Error("자기 자신에게는 친구 요청을 보낼 수 없습니다.");
        }
    }
}

exports.sendFriendsRequest = onCall(fnConfig, async (request) => {
    const sender = new SendFriendsRequest();
    return sender.Operation(request);
});
