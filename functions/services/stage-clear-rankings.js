const admin = require('../admin');

const rankingService = {
    stageClearRanking: async (stageId, uid, clearTime, remainAP, replayId) => {
        const rankingRef = admin.firestore().collection("rankings").doc(stageId);

        await admin.firestore().runTransaction(async (tx) => {
            const doc = await tx.get(rankingRef);
            let data = doc.exists ? doc.data() : {};

            data[uid] = { clearTime, remainAP, replayId };

            let arr = Object.entries(data).map(([userUid, d]) => ({ userUid, ...d }));
            arr.sort((a, b) => a.clearTime - b.clearTime);
            arr = arr.slice(0, 10);

            const top10 = {};
            arr.forEach((item) => {
                top10[item.userUid] = {
                    clearTime: item.clearTime,
                    remainAP: item.remainAP,
                    replayId: item.replayId,
                };
            });

            tx.set(rankingRef, top10);
        });
    }
};

module.exports = { rankingService };