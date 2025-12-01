const admin = require("../admin");

const stageReplayService = {
    stageClearReplay: async (replayId, replayData, stageId, uid) => {
        const replayRef = admin.firestore().collection("replays").doc(replayId);
        await replayRef.set({
            replayData,
            stageId,
            userUid: uid,
            timestamp: admin.firestore.FieldValue.serverTimestamp(),
        });
        return replayRef.id;
    }
};

module.exports = { stageReplayService };