const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

const minute = 1;
const rechargeInterval = minute * 60 * 1000;
const maxTicket = 10;

exports.rechargeTicket = onCall(fnConfig, async (request) => {
  const uid = request.auth.uid;
  if (!uid) {
    return {success: false, reason: "로그인이 필요합니다."};
  }

  const userPrivateRef = admin.firestore().collection("users").doc(uid).collection("private").doc("data");
  const now = Date.now();

  try {
    const result = await admin.firestore().runTransaction(async (t) => {
      const userDoc = await t.get(userPrivateRef);
      if (!userDoc.exists) {
        throw new Error("사용자를 찾을 수 없습니다.");
      }

      const data = userDoc.data();
      const currentTicket = data.gameTicket || 0;
      const lastTime = data.lastTicketTime || Date.now();

      if (currentTicket >= maxTicket || !lastTime) {
        return {
          success: true,
          gameTicket: currentTicket,
          bonusTicket: data.bonusTicket || 0,
          lastTicketTime: lastTime,
          added: 0,
        };
      }

      const elapsedTime = now - lastTime;
      const addCount = Math.floor(elapsedTime / rechargeInterval);

      if (addCount <= 0) {
        return {
          success: true,
          gameTicket: currentTicket,
          bonusTicket: data.bonusTicket || 0,
          lastTicketTime: lastTime,
          added: 0,
        };
      }

      const newRegenTicket = Math.min(currentTicket + addCount, maxTicket);
      const ticketsActuallyAdded = newRegenTicket - currentTicket;

      let newLastTime = lastTime;

      if (newRegenTicket < maxTicket) {
        newLastTime = lastTime + (ticketsActuallyAdded * rechargeInterval);
      } else {
        newLastTime = 0;
      }

      t.update(userPrivateRef, {
        gameTicket: newRegenTicket,
        bonusTicket: data.bonusTicket || 0,
        lastTicketTime: newLastTime,
      });

      return {
        success: true,
        gameTicket: newRegenTicket,
        bonusTicket: data.bonusTicket || 0,
        lastTicketTime: newLastTime,
        serverTime: now,
        added: ticketsActuallyAdded,
      };
    });

    return result;
  } catch (error) {
    console.error("rechargeTicket 트랜잭션 실패:", error.message);
    return {success: false, reason: error.message || "티켓 충전 중 오류 발생"};
  }
});
