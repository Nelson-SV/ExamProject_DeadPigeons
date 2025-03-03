import {atom} from "jotai";
import {ProcessedWinningSequence} from "/src/Api.ts";
import {atomWithStorage} from "jotai/utils";
import {StorageKeys} from "/src/helpers/StorageKeys.ts";


const checkIfWiningNumbersAreInserted = (): boolean => {
    const session = sessionStorage.getItem(StorageKeys.WIN_SEQUENCE);
    return session ? session.length > 0 : false;
};

export const WinningSequenceResponse = atom<ProcessedWinningSequence>({
    registered: checkIfWiningNumbersAreInserted(),
    message: "",
});




interface DisabledInput{
    disabled:boolean
}

export const DisabledSequenceInput = atomWithStorage<DisabledInput>(StorageKeys.INPUT_DISABLED, {disabled:false}, {
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
})