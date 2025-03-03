import {useAtom, useEffect, UserTicketHistoryAtom, http, PaginationAtom, getUserInfoFromToken} from "./imports";
import toast from "react-hot-toast";

export function useInitializeHistory({page = 1}) {

    const [, setUserTickets] = useAtom(UserTicketHistoryAtom);
    const [, setTotalPages] = useAtom(PaginationAtom);

    const userId: string = getUserInfoFromToken().userId;
    const pageSize = 7;

    useEffect(() => {
        http.api.userGetUserTicketsHistory({userId, page, pageSize})
            .then((response) => {
                setUserTickets(response.data.items);
                setTotalPages(Math.ceil(response.data.totalItems / pageSize));
        }).catch(e => {
            const message = e.response?.data?.message || "An unexpected error occurred.";
            toast.error(`Error: ${message}`);
        });
    }, [page, pageSize]);
}