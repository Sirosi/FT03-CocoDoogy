const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

exports.clearStage = onCall(fnConfig, async (request) => {
    const uid = request.auth.uid;
    if (!uid) return { success: false, reason: "로그인이 필요합니다." };

    const { theme, level, clearTime, remainAP, replayData } = request.data;
    console.log("theme:", theme, "level:", level, "clearTime:", clearTime, "remainAP:", remainAP);
    if (!theme || !level || clearTime === undefined || remainAP === undefined) { // replayData 추가하면 이것도 예외체크 필요
        return { success: false, reason: "필수 파라미터가 누락되었습니다." };
    }

    const stageId = `${theme}${level}`;
    const stageRef = admin.firestore()
        .collection("users")
        .doc(uid)
        .collection("stageInfo")
        .doc(stageId);

    try {
        await admin.firestore().runTransaction(async (transaction) => {
        const doc = await transaction.get(stageRef);

        const newData = {
            clearTime,
            remainAP,
            theme,
            level,
            // replayData: JSON.stringify(replayData),
        };

        if (!doc.exists) {

            transaction.set(stageRef, newData);
            return;
        }

        const oldData = doc.data();

        if (!oldData.clearTime || clearTime < oldData.clearTime) {
            transaction.set(stageRef, newData, { merge: true });
        }
    });

    return { success: true };

    } catch (error) {
        console.error("clearStage 오류:", error.message);
        return { success: false, reason: "스테이지 클리어 기록 저장 중 오류 발생" };
    }
});