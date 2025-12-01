const admin = require("../admin");

const stageRecordService = {
    stageClearRecord: async (transaction, uid, stageId, clearTime, remainAP, replayId, theme, level, star) => {
        const stageRef = admin.firestore()
            .collection("users")
            .doc(uid)
            .collection("stageInfo")
            .doc(stageId);

        const doc = await transaction.get(stageRef);
        const newData = { clearTime, remainAP, replayId, theme, level, star };

        if (!doc.exists) {
            transaction.set(stageRef, newData);
            return;
        }
        const oldData = doc.data();
        if (!oldData.clearTime || clearTime < oldData.clearTime || star > oldData.star) {
            transaction.set(stageRef, newData, { merge: true });
        }
    }
};

module.exports = { stageRecordService };