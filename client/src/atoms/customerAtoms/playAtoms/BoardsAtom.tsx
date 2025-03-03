import { atom } from "jotai/index";

export const BoardsAtom = atom([
    {
        buttons: new Array(16).fill(false),
        sequence: [] as number[],
        price: null as number | null
    },
]);
