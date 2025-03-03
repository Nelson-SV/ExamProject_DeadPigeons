import { atomWithStorage } from "jotai/utils";
import {CurrentGameDto} from "/src/Api.ts";
import {StorageKeys} from "/src/helpers";

export const CurrentGameAtom = atomWithStorage<CurrentGameDto | null>(
    StorageKeys.GAME_INFO,
    null,
    {
        getItem: (key) => {
            const storedValue = sessionStorage.getItem(key);
            return storedValue ? JSON.parse(storedValue) : null;
        },
        setItem: (key, value) => {
            if (value) {
                sessionStorage.setItem(key, JSON.stringify(value));
            } else {
                sessionStorage.removeItem(key);
            }
        },
        removeItem: (key) => {
            sessionStorage.removeItem(key);
        },
    }
);


