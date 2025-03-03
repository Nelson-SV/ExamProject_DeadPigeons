import {atom} from "jotai";
import {CurrentBalanceDto} from "/src/Api.ts"



export const BalanceAtom = atom<CurrentBalanceDto>({balanceValue:0});