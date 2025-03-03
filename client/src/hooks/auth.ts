import {useNavigate} from "react-router-dom";
import {atom, useAtom} from "jotai";
import {AuthUserInfo} from "/src/Api.ts";
import {StorageKeys} from "/src/helpers/StorageKeys.ts";
import {http} from "/src/helpers";
import {atomWithStorage, createJSONStorage} from "jotai/utils";
import {CurrentGameAtom, WinningSequenceAtom, WinningSequenceResponse} from "/src/atoms";
import {useAtomValue} from "jotai/index";


export const tokenStorage = createJSONStorage<string | null>(
    () => sessionStorage,
);

export const jwtAtom = atomWithStorage<string | null>(StorageKeys.TOKEN_KEY, null, tokenStorage);

const userInfoAtom = atom(async (get) => {
    const token = get(jwtAtom);
    if (!token) return null;
    const response = await http.api.authUserInfo();
    return response.data;
});

export type Credentials = { email: string; password: string };

type AuthHook = {
    user: AuthUserInfo | null;
    login: (credentials: Credentials) => Promise<void>;
    logout: () => void;
    logoutAdmin: () => void;
};

export const useAuth = () => {
    const [_, setJwt] = useAtom(jwtAtom);
    const [, setCurrentGame] = useAtom(CurrentGameAtom);
    const [, setWinningSequence] = useAtom(WinningSequenceAtom);
    const [,setProcessedWinningSequence] = useAtom(WinningSequenceResponse);
    const [user] = useAtom(userInfoAtom);
    const navigate = useNavigate();

    const login = async (credentials: Credentials) => {
        const response = await http.api.authLogin(credentials);
        const data = response.data;
        setJwt(data.jwt!);
    };

    const logout = async () => {
        setJwt(null);
        navigate("/")
    };

    const logoutAdmin = async () => {
        setJwt(null);
        setCurrentGame(null);
        setWinningSequence([]);
        sessionStorage.clear();
        setProcessedWinningSequence({registered:false,message:""});
        navigate("/");
    }

    return {
        user,
        login,
        logout,
        logoutAdmin
    } as AuthHook;
};

