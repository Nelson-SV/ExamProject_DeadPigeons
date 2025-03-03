import {createJSONStorage} from "jotai/utils";
import {CreateTicketDto, CurrentGameDto} from "/src/Api.ts";


export const createSessionStorageArray = createJSONStorage<number[]>(
    () => sessionStorage,
);

export const createSessionStorageCurrentGame = createJSONStorage<CurrentGameDto | null>(
    () => sessionStorage,
);
export const createSessionStoragePlay = createJSONStorage<CreateTicketDto[] >(
    () => sessionStorage,
);
