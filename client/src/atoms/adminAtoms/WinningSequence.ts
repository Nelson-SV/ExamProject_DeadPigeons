import {atomWithStorage} from "jotai/utils";
import {createSessionStorageArray, StorageKeys} from "/src/helpers";



export const WinningSequenceAtom = atomWithStorage<number[]>(StorageKeys.WIN_SEQUENCE, [], createSessionStorageArray);