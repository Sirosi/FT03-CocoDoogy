const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");
const Request = require("../request/Request");

class TakeAllPresentRequest extends Request {
    async Operation(request) {
    const uid = request.auth.uid;

    return this.executeTransaction(uid, uid, async (transaction, refs, docs) => {
        const userData = docs.userPrivateData;
        const giftList = userData.giftList || [];

        if (giftList.length === 0)
            throw new Error("수령할 선물이 없습니다.");

        const rewardSummary = {
            bonusTicket: 0,

        };

        for (const gift of giftList) {
            const giftType = gift.giftType;
            const giftCount = gift.giftCount || 1;

            switch (giftType) {
                case "bonusTicket":
                    rewardSummary.bonusTicket += giftCount;
                    break;

                default:
                    throw new Error(`알 수 없는 선물 타입: ${giftType}`);
            }
        }

        const updateData = {
            giftList: [],
            bonusTicket: (userData.bonusTicket || 0) + rewardSummary.bonusTicket,
        };

        transaction.update(refs.userPrivateRef, updateData);

        return `모든 선물을 수령했습니다.\n 총 ${rewardSummary.bonusTicket}개의 선물을 받았습니다.`;
    });
}

    validateBeforeTake(gift) {
        if (!gift) throw new Error("해당 선물이 존재하지 않습니다.");
        if (gift.isClaimed) throw new Error("이미 받은 선물입니다.");
    }
}

exports.takeAllPresentRequest = onCall(fnConfig, async (request) => {
    const taker = new TakeAllPresentRequest();
    return taker.Operation(request);
});
