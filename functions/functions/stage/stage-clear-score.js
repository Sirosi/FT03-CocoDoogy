const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

exports.stageClearScore = onCall(fnConfig, async (request) => {
    const uid = request.auth?.uid;
    if (!uid) return { success: false, reason: "로그인이 필요합니다." };

    const data = request.data || {};
    const refillCount = data.refillCount;
    const scores = data.scores;

    console.log("refillCount:", refillCount);
    console.log("scores:", scores);

    if (refillCount === undefined) {
        return { success: false, reason: "필수 파라미터가 누락되었습니다." };
    }
    if (!Array.isArray(scores)) {
        return { success: false, reason: "scores는 배열이어야 합니다." };
    }

    const getStars = (refill) => {
        for (let i = 0; i < scores.length; i++) {
                if (refill <= scores[i])
                    {
                        return scores.length - i;
                    }
                }
            return 1;
        };

    const stars = getStars(refillCount);
    return { success: true, stars };
});