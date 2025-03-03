import {atom} from "jotai";
import {getUserInfoFromToken} from "/src/helpers";


export const RoleLoginAtom = atom(async () => {
    const storedRole = getUserInfoFromToken().role;
    return storedRole || "";
})




