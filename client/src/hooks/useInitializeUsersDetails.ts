import {useAtom, useEffect, UsersDetailsAtom, http, PaginationAtom, getUserInfoFromToken} from "./imports";
import toast from "react-hot-toast";

export function useInitializeUsersDetails({page = 1}) {

    const [, setUsersDetails] = useAtom(UsersDetailsAtom);
    const [, setTotalPages] = useAtom(PaginationAtom);

    const adminId: string = getUserInfoFromToken().userId;
    const pageSize = 7;

    useEffect(() => {
        http.api.adminUserManagementGetUsersDetails({adminId, page, pageSize})
            .then((response) => {
                setUsersDetails(response.data.items);
                setTotalPages(Math.ceil(response.data.totalItems / pageSize));
            }).catch(e => {
            const message = e.response?.data?.message || "An unexpected error occurred.";
            toast.error(`Error: ${message}`);
        });
    }, [page, pageSize]);
}