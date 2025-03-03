import {atom, useAtom} from "jotai";
import {CurrentGameDto} from "/src/Api.ts";

export const RetrievedGamesAtom =  atom<CurrentGameDto[]>([])