const {onCall} = require("firebase-functions/v2/https");
const {fnConfig} = require("../../config");
const admin = require("../../admin");

exports.useItem = onCall(fnConfig, async (request) => {
    const uid = request.auth.uid;
    const itemId = request.data.itemId;
    const itemQuantity = 1; // 아이템은 무조건 1개씩 사용하도록 고정
    if (!uid) {
        return {success: false, reason: "로그인이 필요합니다."};
    }
    if (!itemId) {
        return {success: false, reason: "아이템 ID가 불일치하거나 존재하지 않습니다."};
    }
    try {
        const userPrivateRef = admin.firestore().collection("users").doc(uid).collection("private").doc("data");
        const userPrivateDoc = await userPrivateRef.get();
        if (!userPrivateDoc.exists) {
            return {success: false, reason: "사용자 데이터를 찾을 수 없습니다."};
        }
        const userPrivateData = userPrivateDoc.data();
        const userInventory = userPrivateData.itemDic || {};

        if (!userInventory[itemId] || userInventory[itemId] < itemQuantity) {
            return {success: false, reason: "인벤토리에 해당 아이템이 부족합니다."};
        }

        userInventory[itemId] -= itemQuantity;

        await userPrivateRef.update({itemDic: userInventory});

        return {success: true, message: "아이템이 성공적으로 사용되었습니다."};
    } catch (err) {
        console.error("useItem error:", err);
        return {success: false, reason: "아이템 사용 중 오류가 발생했습니다."};
    }
});