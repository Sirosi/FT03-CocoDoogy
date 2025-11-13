const admin = require("../admin");
const db = admin.firestore();

exports.processPurchase = async (uid, itemId, itemQuantity, moneyType = "inGameMoney") => {
  const userRef = db.collection("users").doc(uid).collection("private").doc("data");
  const itemRef = db.collection("items").doc(itemId);

  try {
    const itemSnap = await itemRef.get();
    if (!itemSnap.exists) {
      return { success: false, reason: "아이템을 찾을 수 없습니다." };
    }

    const itemData = itemSnap.data();
    const price = itemData.price * itemQuantity;

    console.log(
      `Processing purchase for uid: ${uid}, itemId: ${itemId}, quantity: ${itemQuantity}, price: ${price}`
    );

    if (price < 0) { // (!price || price < 0)
      return { success: false, reason: "잘못된 아이템 가격입니다." };
    }

    const result = await db.runTransaction(async (t) => {
      const doc = await t.get(userRef);
      const userData = doc.data() || {};
      const itemType = itemData.itemType;

      const currentMoney = userData[moneyType] || 0;
      if (currentMoney < price) {
        throw new Error("잔액 부족");
      }

      const costDelta = -price;
      const itemDelta = (itemType === "gameTicket" || itemType === "cashMoney")
        ? itemData.value * itemQuantity
        : 0;

      const updates = {};

      if (itemType === moneyType) {
        updates[moneyType] = admin.firestore.FieldValue.increment(costDelta + itemDelta);
      } else {
        updates[moneyType] = admin.firestore.FieldValue.increment(costDelta);

            if (itemDelta) {
          updates[itemType] = admin.firestore.FieldValue.increment(itemDelta);
        } else {
          const currentInventory = userData.itemDic || {};
          const newCount = (currentInventory[itemId] || 0) + itemQuantity;
          updates[`itemDic.${itemId}`] = newCount;
        }
      }
      t.update(userRef, updates);

      return { newValue: itemDelta || updates[`itemDic.${itemId}`], remaining: currentMoney + costDelta, itemType };
    });

    return {
      success: true,
      message: "성공적으로 아이템을 구매했습니다.",
      data: {
        uid,
        itemId,
        price,
        count: result.newValue,
        remaining: result.remaining,
        storedIn: result.itemType,
      },
    };
  } catch (err) {
    console.error("processPurchase error:", err);
    return {
      success: false,
      reason: err.message || "아이템 구매 중 오류가 발생했습니다.",
    };
  }
};
