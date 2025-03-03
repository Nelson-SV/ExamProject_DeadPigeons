import {StorageKeys} from "/src/helpers/StorageKeys.ts";


interface UserInfoFromToken {
    userId: string,
    username: string,
    email: string,
    role: string
}


export const getUserInfoFromToken = (): UserInfoFromToken => {
    const userInfoEmpty: UserInfoFromToken = {
        userId: "",
        username: "",
        email: "",
        role: ""
    }
    const token = sessionStorage.getItem(StorageKeys.TOKEN_KEY);
    if (token === null) {
        return userInfoEmpty;
    }

    try {
        const payload = token.split(".")[1];
        const decoded = JSON.parse(atob(payload));
        const userId = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        const username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        const email = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
        const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        return {userId, username, email, role};
    } catch (error) {
        console.error("Invalid token", error);
        return userInfoEmpty;
    }
};