import {atom} from "jotai";
import {AutomatedTicketsDto} from "/src/Api.ts"
export const WeeklyBoardsAtom = atom<AutomatedTicketsDto[]>([]);