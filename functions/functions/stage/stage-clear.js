const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

const { stageReplayService } = require("../../services/stage-clear-replays");
const { stageRecordService } = require("../../services/stage-clear-userrecord");
const { rankingService } = require("../../services/stage-clear-rankings");

exports.stageClear = onCall(fnConfig, async (request) => {
    const uid = request.auth?.uid;
    if (!uid) return { success: false, reason: "로그인이 필요합니다." };

    const data = request.data || {};
    const { theme, level, clearTime, remainAP, replayData } = data;

    if (!theme || !level || clearTime === undefined || remainAP === undefined || replayData === undefined) {
        return { success: false, reason: "필수 파라미터가 누락되었습니다." };
    }

    const stageId = `${theme}${level}`;
    const replayId = `replay_${stageId}_${uid}`;

    try {

        await stageReplayService.stageClearReplay(replayId, replayData, stageId, uid);


        await admin.firestore().runTransaction(async (transaction) => {
            await stageRecordService.stageClearRecord(
                transaction,
                uid,
                stageId,
                clearTime,
                remainAP,
                replayId,
                theme,
                level
            );
        });

        await rankingService.stageClearRanking(
            stageId,
            uid,
            clearTime,
            remainAP,
            replayId
        );

        return { success: true };
    } catch (e) {
        console.error("clearStage 오류:", e);
        return { success: false, reason: e.message };
    }
});
