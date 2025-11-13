const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class TakePresentRequest extends Request {
    async Operation(request) {
        const uid = request.auth.uid;
        const giftId = request.data.giftId;

        if (!giftId) throw new Error("giftId가 필요합니다.");

        return this.executeTransaction(uid, uid, async (transaction, refs, docs) => {
            const userData = docs.userPrivateData;
            const giftList = userData.giftList || [];

            const giftIndex = giftList.findIndex(g => g.giftId === giftId);
            if (giftIndex === -1) throw new Error("해당 선물을 찾을 수 없습니다.");

            const gift = giftList[giftIndex];
            if (gift.isClaimed) throw new Error("이미 수령한 선물입니다.");

            const updateData = {};
            const giftType = gift.giftType;
            const giftCount = gift.giftCount || 1;

            switch (giftType) {
                case "bonusTicket":
                    updateData.bonusTicket = (userData.bonusTicket || 0) + giftCount;
                    break;
                // TODO : 나중에 더 추가 할수도?
                default:
                    throw new Error(`알 수 없는 선물 타입: ${giftType}`);
            }

            const updatedGiftList = [...giftList];
            updatedGiftList.splice(giftIndex, 1);

            updateData.giftList = updatedGiftList;

            transaction.update(refs.userPrivateRef, updateData);

            return "선물을 수령했습니다.";
        });
    }

    validateBeforeTake(gift) {
        if (!gift) throw new Error("해당 선물이 존재하지 않습니다.");
        if (gift.isClaimed) throw new Error("이미 받은 선물입니다.");
    }
}

exports.takePresentRequest = onCall(fnConfig, async (request) => {
    const taker = new TakePresentRequest();
    return taker.Operation(request);
});
