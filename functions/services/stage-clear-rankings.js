const admin = require('../admin');

const MAX_RANKINGS = 10;

const rankingService = {
    stageClearRanking: async (stageId, uid, clearTime, remainAP, replayId, refillPoints) => {
        const rankingRef = admin.firestore().collection("rankings").doc(stageId);
        const userRef = admin.firestore().collection("users").doc(uid).collection("public").doc("profile");

        await admin.firestore().runTransaction(async (transaction) => {
            const doc = await transaction.get(rankingRef);
            let data = doc.exists ? doc.data() : {};

            const userDoc = await transaction.get(userRef);
            const nickname = userDoc.exists ? userDoc.data().nickName || "" : "";

            if (!data[uid] || clearTime < data[uid].clearTime) {
                data[uid] = { clearTime, remainAP, replayId, refillPoints, nickname};
            }

            let arr = Object.entries(data).map(([userUid, d]) => ({ userUid, ...d }));
            arr.sort((a, b) => {
                // 1. 초기화(refillPoints) 적은 순서
                if (a.refillPoints !== b.refillPoints) {
                    return a.refillPoints - b.refillPoints;
                }

                // 2. 행동력(remainAP) 적은 순서
                if (a.remainAP !== b.remainAP) {
                    return a.remainAP - b.remainAP;
                }

                // 3. 클리어 시간(clearTime) 짧은 순서
                return a.clearTime - b.clearTime;
            });

            arr = arr.slice(0, MAX_RANKINGS);

            const top10 = {};
            let prevTime = null;
            let rank = 0;
            let sameRankCount = 0;

            arr.forEach((item) => {
                if (item.clearTime === prevTime) {
                    sameRankCount++;
                } else {
                    rank = rank + 1 + sameRankCount;
                    sameRankCount = 0;
                }
                prevTime = item.clearTime;

                top10[item.userUid] = {
                    rank: rank,
                    clearTime: item.clearTime,
                    remainAP: item.remainAP,
                    replayId: item.replayId,
                    refillPoints: item.refillPoints,
                    nickname: item.nickname
                };
            });

            transaction.set(rankingRef, top10);
        });
    }
};

module.exports = { rankingService };