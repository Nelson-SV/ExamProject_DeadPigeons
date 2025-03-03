import { atom } from "jotai";
import {CreateTicketDto} from "/src/Api.ts"
import {atomWithStorage} from "jotai/utils";
import {StorageKeys} from "/src/helpers";
import {createSessionStoragePlay} from "/src/helpers/CreateSesionStorage.ts";


export const AddedBoardsAtom = atomWithStorage<CreateTicketDto[] >(StorageKeys.ADDED_BOARDS, [], createSessionStoragePlay );
