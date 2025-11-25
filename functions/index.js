const {purchaseWithInGameMoney} = require(
    "./functions/purchase/purchaseWithInGameMoney",
);
const {purchaseWithCashMoney} = require(
    "./functions/purchase/purchaseWithCashMoney",
);
const {rechargeTicket} = require(
    "./functions/ticket/rechargeTicket",
);
const {consumeTicket} = require(
    "./functions/ticket/consumeTicket",
);
const {takeInGameMoney} = require(
    "./functions/take/takeInGameMoney",
);
const {sendFriendsRequest} = require(
    "./functions/friends/sendFriendsRequest",
);
const {receiveFriendsRequest} = require(
    "./functions/friends/receiveFriendsRequest",
);
const {rejectFriendsRequest} = require(
    "./functions/friends/rejectFriendsRequest",
);
const {cancelFriendsRequest} = require(
    "./functions/friends/cancelFriendsRequest",
);
const {deleteFriendsRequest} = require(
    "./functions/friends/deleteFriendsRequest",
);
const {giftFriendsRequest} = require(
    "./functions/friends/giftFriendsRequest",
);
const {takePresentRequest} = require(
    "./functions/take/takePresentRequest",
);
const {useItem} = require(
    "./functions/item/useItem",
);
const {clearStage} = require(
    "./functions/stage/clearStage",
);

exports.clearStage = clearStage;
exports.takeInGameMoney = takeInGameMoney;

exports.purchaseWithInGameMoney = purchaseWithInGameMoney;
exports.purchaseWithCashMoney = purchaseWithCashMoney;

exports.rechargeTicket = rechargeTicket;
exports.consumeTicket = consumeTicket;

exports.sendFriendsRequest = sendFriendsRequest;
exports.receiveFriendsRequest = receiveFriendsRequest;
exports.rejectFriendsRequest = rejectFriendsRequest;
exports.cancelFriendsRequest = cancelFriendsRequest;
exports.deleteFriendsRequest = deleteFriendsRequest;
exports.giftFriendsRequest = giftFriendsRequest;
exports.takePresentRequest = takePresentRequest;
exports.useItem = useItem;