
import {useAtom} from "jotai/index";
import {useInitializePayments} from "/src/components/imports.ts";
import React, {useState} from "react";
import {PaymentsAtom} from "/src/atoms/adminAtoms/paymentsAtoms/PaymentsAtom.tsx";
import {PaymentsPaginationAtom} from "/src/atoms/adminAtoms/paymentsAtoms/PaymentsPaginationAtom.tsx";
import Pagination from "/src/components/shared/Pagination.tsx";
import {DeclinePaymentDto, PaymentDto, UpdatePaymentDto} from "/src/Api.ts";
import { http } from "/src/helpers";
import toast from "react-hot-toast";
import {AxiosError} from "axios";


export default function Payments  () {
    const [totalPages] = useAtom(PaymentsPaginationAtom);
    const [payments, setPayments] = useAtom(PaymentsAtom);
    const [currentPage, setCurrentPage] = useState(1);

    useInitializePayments({page: currentPage});

    const handleApprove = async (payment : PaymentDto) => {
        try {
            const updatePaymentDto: UpdatePaymentDto = {
                guid: payment.guid,
                value: payment.value,
                userId: payment.userId,
            };
            const response = await http.api.paymentUpdatePendingPayments(updatePaymentDto);
            if (response.status === 200) {
                toast.success(`Payment updated successfully for user ${payment.userName}`);
                setPayments((prevPayments) =>
                    prevPayments.filter((p) => p.guid !== payment.guid)
                );
            }

        }
        catch (e) {
            if (e instanceof AxiosError) {
                const errorData = e.response?.data; /* Access the data field in the response*/
                if (errorData) {
                    if (errorData) {
                        toast.error(errorData);
                    } else {
                        toast.error("An unexpected error occurred. Please try again later.");
                    }
                }
            } else {
                toast.error("Network error. Please try again later.");
            }
        }

    }
    /* Declines a pending payment and notifies the user via email. Creates a DeclinePaymentDto object with the payment details and sends it to the server to decline the payment.
    On success, it removes the declined payment from the list and shows a success toast indicating a notification email was sent */
    const handleDecline = async (payment : PaymentDto) => {
        try {
            const declinePaymentDto: DeclinePaymentDto = {
                guid: payment.guid,
                name: payment.name,
                userId: payment.userId,
                bucket: payment.bucket,
                userName: payment.userName
            };
            const response = await http.api.paymentDeclinePendingPayments(declinePaymentDto);
            if (response.status === 200) {
                toast.success(`notification email sent to user ${payment.userName}`);
                setPayments((prevPayments) =>
                    prevPayments.filter((p) => p.guid !== payment.guid)
                );
            }

        }
        catch (e) {
            if (e instanceof AxiosError) {
                const errorData = e.response?.data; /* Access the data field in the response*/
                if (errorData) {
                    if (errorData) {
                        toast.error(errorData);
                    } else {
                        toast.error("An unexpected error occurred. Please try again later.");
                    }
                }
            } else {
                toast.error("Network error. Please try again later.");
            }
        }
    }

    return (
        <div className="flex flex-col mt-24 sm:w-1/2 md:w-2/4 lg:w-3/4 xl:w-3/5 mx-auto">
            {Array.isArray(payments) && payments.length === 0 ? (
                <p className="text-sm sm:text-base md:text-lg lg:text-xl">
                    No payments found
                </p>
            ) : (
                Array.isArray(payments) &&
                payments.map((payments, index) => (
                    <div
                        key={payments.guid || index}
                        tabIndex={0}
                        className="collapse mb-4 collapse-arrow bg-[#dddddd] border"
                    >
                        {/* Title container with flex properties to center spans and buttons */}
                        <div
                            className="collapse-title flex flex-wrap gap-4 justify-center items-center text-sm sm:text-base md:text-lg lg:text-lg font-medium ">
                            <span className="min-w-[150px] flex-grow text-center truncate sm:whitespace-normal ">{payments.userName}</span>
                            <span className="min-w-[150px] flex-grow text-center">
                        Date Created: {new Date(payments.timeCreated).toLocaleDateString()}
                    </span>
                            <span className="min-w-[150px] flex-grow text-center"> DKK {payments.value}</span>

                            {/* Buttons wrapper */}
                            <div
                                className="flex flex-wrap gap-6 justify-center w-full sm:w-auto">
                                <button
                                    onClick={() => handleApprove(payments)}
                                    className="py-2 px-4 sm:px-6 lg:px-8 text-sm sm:text-base lg:text-lg bg-blue-400  text-white font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300"
                                >
                                    Approve
                                </button>
                                <button
                                    onClick={() => handleDecline(payments)}

                                    className="py-2 px-4 sm:px-6 lg:px-8 text-sm sm:text-base lg:text-lg bg-red-600 text-white font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300"
                                >
                                    Decline
                                </button>
                            </div>
                        </div>

                        {/* Collapse content with transaction info */}
                        <div className="collapse-content ml-3 text-sm sm:text-base md:text-lg lg:text-lg font-medium">
                            <img src={payments.mediaLink} />
                            <span className="min-w-[150px] text-center">
                               Transaction number: {payments.transactionId}
                            </span>
                        </div>

                    </div>
                ))
            )}

            <Pagination
                totalPages={totalPages}
                currentPage={currentPage}
                onPageChange={setCurrentPage}
            />
        </div>


    );


}
