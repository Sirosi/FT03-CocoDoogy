const admin = require("../../admin");

class Request {
    constructor(fnConfig) {
        this.admin = admin;
        this.fnConfig = fnConfig;
    }

    validateRequest(uid, fuid) {
        if (!uid) throw new Error("로그인이 필요합니다.");
        if (!fuid) throw new Error("해당 닉네임을 가진 유저가 없습니다.");
    }

    getReferences(uid, fuid) {
        return{
            userPrivateRef: this.admin.firestore().collection("users").doc(uid).collection("private").doc("data"),
            friendsPrivateRef: this.admin.firestore().collection("users").doc(fuid).collection("private").doc("data"),
            userPublicRef: this.admin.firestore().collection("users").doc(uid).collection("public").doc("profile"),
            friednsPublicRef: this.admin.firestore().collection("users").doc(fuid).collection("public").doc("profile"),
        }
    }

    async executeTransaction(uid, fuid, transactionLogic, needProfiles = false) {
        try{
            this.validateRequest(uid, fuid);
            const refs = this.getReferences(uid, fuid);

            const result = await admin.firestore().runTransaction(async (transaction) => {
                const docs = await this.getDocuments(transaction, refs, needProfiles);
                return await transactionLogic(transaction, refs, docs);
            });
            return { success: true, message: result };
        }catch(err){
            console.error(`${this.constructor.name} error:`, err);
            return { success: false, reason: err.message };
        }
    }

    async getDocuments(transaction, refs, needProfiles) {
        let userPrivateDoc, friendsPrivateDoc;

        if (transaction) {
            [userPrivateDoc, friendsPrivateDoc] = await Promise.all([
                transaction.get(refs.userPrivateRef),
                transaction.get(refs.friendsPrivateRef)
            ]);
        } else {
            [userPrivateDoc, friendsPrivateDoc] = await Promise.all([
                refs.userPrivateRef.get(),
                refs.friendsPrivateRef.get()
            ]);
        }

        if (!userPrivateDoc.exists || !friendsPrivateDoc.exists) {
            throw new Error("사용자를 찾을 수 없습니다.");
        }

        const docs = {
            userPrivateData: userPrivateDoc.data(),
            friendsPrivateData: friendsPrivateDoc.data()
        };

        if (needProfiles) {
            let userPublicDoc, friendsPublicDoc;
            if (transaction) {
                [userPublicDoc, friendsPublicDoc] = await Promise.all([
                    transaction.get(refs.userPublicRef),
                    transaction.get(refs.friednsPublicRef)
                ]);
            } else {
                [userPublicDoc, friendsPublicDoc] = await Promise.all([
                    refs.userPublicRef.get(),
                    refs.friednsPublicRef.get()
                ]);
            }

            if (!userPublicDoc.exists || !friendsPublicDoc.exists) {
                throw new Error("사용자 프로필을 찾을 수 없습니다.");
            }

            docs.userPublicProfile = userPublicDoc.data();
            docs.friendsPublicProfile = friendsPublicDoc.data();
        }

        return docs;
    }
}

module.exports = Request;
