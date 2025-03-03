import SequenceDisplay from "./SequenceDisplay.tsx";
import Pagination from "/src/components/shared/Pagination.tsx";
import React, {useState} from "react";
import {useAtom} from "jotai";
import {UserTicketHistoryAtom, useInitializeHistory, PaginationAtom} from "/src/components/imports.ts";


export default function HistoryPage() {

    const [sortConfig, setSortConfig] = useState({ key: null, direction: "default" });
    const [gameTickets, setGameTickets] = useAtom(UserTicketHistoryAtom);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages] = useAtom(PaginationAtom);

    useInitializeHistory({page: currentPage});

    const sortedData = React.useMemo(() => {
        if (!sortConfig.key) return gameTickets;

        const sorted = [...gameTickets].sort((a, b) => {
            if (!sortConfig.key) return 0;

            const key = sortConfig.key;

            if (a[key] < b[key]) return sortConfig.direction === "asc" ? -1 : 1;
            if (a[key] > b[key]) return sortConfig.direction === "asc" ? 1 : -1;

            return 0;
        });

        return sorted;
    }, [gameTickets, sortConfig]);

    const handleSort = (key) => {
        setSortConfig((prevConfig) => {
            if (prevConfig.key === key) {
                const nextDirection = prevConfig.direction === "asc" ? "desc" : prevConfig.direction === "desc" ? "default" : "asc";
                return { key: nextDirection === "default" ? null : key, direction: nextDirection };
            }
            return { key, direction: "asc" };
        });
    };

    return (
        <div className="overflow-x-auto px-4 sm:px-32 py-6">
            <table className="table-auto w-full border-separate border-spacing-y-2">
                <thead className="hidden sm:table-header-group">
                <tr>
                    <th
                        className="w-32 sm:w-52 text-sm sm:text-base cursor-pointer text-black text-left pl-4"
                        onClick={() => handleSort("ticketNumber")}
                    >
                        BILLET NR.
                    </th>
                    <th
                        className="w-32 sm:w-52 text-sm sm:text-base cursor-pointer text-black text-left pl-4"
                        onClick={() => handleSort("purchaseDate")}
                    >
                        KØBSDATO
                    </th>
                    <th className="w-40 sm:w-96 text-sm sm:text-base text-black text-left pl-4">SEKVENS</th>
                    <th
                        className="w-32 sm:w-44 text-sm sm:text-base cursor-pointer text-black text-left pl-4"
                        onClick={() => handleSort("priceValue")}
                    >
                        PRIS
                    </th>
                </tr>
                </thead>
                <tbody>
                {Array.isArray(sortedData) && sortedData.length > 0 ? (
                    sortedData.map((gameTicket, index) => (
                        <tr
                            key={gameTicket.guid + index}
                            className="hover:shadow-md rounded-3xl bg-background flex sm:table-row flex-wrap sm:flex-nowrap mb-4 sm:mb-0"
                        >
                            <td className="p-2 sm:p-4 font-bold rounded-s-2xl w-full sm:w-auto text-left">
                                <span className="sm:hidden text-gray-500 block">Billet Nr:</span>
                                {gameTicket.ticketNumber}
                            </td>
                            <td className="p-2 sm:p-4 font-bold w-full sm:w-auto text-left">
                                <span className="sm:hidden text-gray-500 block">Købsdato:</span>
                                {gameTicket.formattedPurchaseDate}
                            </td>
                            <td className="p-2 sm:p-4 w-full sm:w-auto text-left">
                                <span className="sm:hidden text-gray-500 block">Sekvens:</span>
                                <SequenceDisplay
                                    numbers={gameTicket.sequence}
                                    winningNumbers={gameTicket.extractedNumbers}
                                />
                            </td>
                            <td className="p-2 sm:p-4 font-bold rounded-e-2xl w-full sm:w-auto text-left">
                                <span className="sm:hidden text-gray-500 block">Pris:</span>
                                {gameTicket.priceValue}
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan={4} className="text-center pt-20 text-xl sm:text-4xl">
                            Ingen billetter i din historik
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
            <Pagination
                totalPages={totalPages}
                currentPage={currentPage}
                onPageChange={setCurrentPage}
            />
        </div>
    );
}