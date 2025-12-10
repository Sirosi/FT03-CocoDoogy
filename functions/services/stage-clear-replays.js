const admin = require("../admin");

const stageReplayService = {
    stageClearReplay: async (replayId, replayData, stageId, uid, clearTime, remainAP, star, refillPoints) => {
        const replayRef = admin.firestore().collection("replays").doc(replayId);

        const doc = await replayRef.get();
        const canUpdate = !doc.exists ||
            refillPoints < doc.data().refillPoints ||
            (refillPoints === doc.data().refillPoints && remainAP < doc.data().remainAP) ||
            (refillPoints === doc.data().refillPoints && remainAP === doc.data().remainAP && clearTime < doc.data().clearTime) ||
            (refillPoints === doc.data().refillPoints && remainAP === doc.data().remainAP && clearTime === doc.data().clearTime && star > doc.data().star);


        if(canUpdate){
            await replayRef.set({
                replayData,
                stageId,
                userUid: uid,
                timestamp: admin.firestore.FieldValue.serverTimestamp(),
            });
        }
        return replayRef.id;
    }
};

module.exports = { stageReplayService };