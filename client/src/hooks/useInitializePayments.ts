import {useAtom, useEffect, http} from "./imports";
import {PaymentsAtom} from "/src/atoms/adminAtoms/paymentsAtoms/PaymentsAtom.tsx";
import {PaymentsPaginationAtom} from "/src/atoms/adminAtoms/paymentsAtoms/PaymentsPaginationAtom.tsx";

export function useInitializePayments({page = 1}) {

    const [, setPayments] = useAtom(PaymentsAtom);
    const [, setTotalPages] = useAtom(PaymentsPaginationAtom);

    const pageSize = 5;

    useEffect(() => {
        http.api.paymentGetUserPendingPayments({ page, pageSize})
            .then((response) => {
                setPayments(response.data.items);
                setTotalPages(Math.ceil(response.data.totalItems / pageSize));
            }).catch(e => {
            console.log(e)
        });
    }, [page, pageSize]);
}