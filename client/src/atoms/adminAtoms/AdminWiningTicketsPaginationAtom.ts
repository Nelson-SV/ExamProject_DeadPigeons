import {atomWithStorage} from "jotai/utils";
import {Pagination} from "/src/Api.ts";
import {StorageKeys} from "/src/helpers";

export const AdminWiningTicketsPaginationAtom = atomWithStorage<Pagination>(StorageKeys.WINNING_PLAYER_PAGINATION,


    {currentPageItems: 10, currentPage: 1, nextPage: 2, hasNext: false, totalItems: 0}, {
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
    });

