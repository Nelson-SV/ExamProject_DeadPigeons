import {useState} from "react";
import {CurrentGameDto} from "/src/Test/Api.ts";
import {format} from "date-fns";
import SequenceDisplay from "/src/components/user/historyPage/SequenceDisplay.tsx";
import {
    Pagination as PagingType,
    WinningPlayer, WinningPlayersDto,
    WinningPlayersRequestDto
} from "/src/Api.ts";

import Pagination from "/src/components/shared/Pagination.tsx"
import {http, showErrorToasts} from "/src/helpers";
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";
import {WinningPlayerDisplay} from "/src/components/AdminComponents/adminMainPage/WinningPlayerDisplay.tsx";
import {XCircleIcon} from "@heroicons/react/24/solid";


interface GameData {
    currentGame: CurrentGameDto
}

export const GameDisplay = ({currentGame}: GameData) => {
    const [isOpen, setIsOpen] = useState(false);
    const [currentPagination, setCurrentPagination] = useState<PagingType>({
        currentPageItems: 10,
        currentPage: 1,
        nextPage: 2,
        hasNext: false,
        totalItems: 0
    })
    const [players, setPlayers] = useState<WinningPlayer []>([]);

    const openAccordion = () => {
        if (!isOpen) {
            retrieveWinningPlayers(currentPagination);
        }
        setIsOpen(true);
    }

    const closeAccordion = () => {
        setCurrentPagination({
            currentPageItems: 10,
            currentPage: 1,
            nextPage: 2,
            hasNext: false,
            totalItems: 0
        })
        setIsOpen(false);
    }
    const handlePageChange = (currentPage: number) => {
        const updatedPagination = {
            ...currentPagination,
            currentPage: currentPage
        };
        toast.success(updatedPagination.currentPage + "");
        setCurrentPagination(updatedPagination);
        retrieveWinningPlayers(updatedPagination);
    }


    const retrieveWinningPlayers = (pagination: PagingType) => {

        const requestObject: WinningPlayersRequestDto = {
            gameId: {guid: currentGame.guid},
            winningSequence: currentGame.extractedNumbers,
            pagination: pagination
        }
        http.api.gameHandlerGetWinningPlayers(requestObject)
            .then((r: AxiosResponse<WinningPlayersDto>) => {
                if (r.status === 200) {
                    setCurrentPagination((prev) => ({...prev, totalItems: r.data.pagination!.totalItems}))
                    setPlayers(r.data.winningPlayers!);
                }
            }).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                showErrorToasts(errorData);
            } else {
                toast.error("Network error. Please try again later.");
            }
        })
    }

    return (
        <div className="join pb-3 join-vertical w-full lg:w-3/4 xl:w-3/5 mx-auto mb-4 ">
            <div
                className={`collapse ${isOpen ? "collapse-open" : "collapse-closed"}  border-t-5 border-base-300 border bg-white rounded-md shadow-md   hover:bg-blue-300 hover:scale-105 transition-transform duration-200`}
                onClick={(e) => {
                    e.stopPropagation();
                    openAccordion()
                }}
            >
                {isOpen && (
                    <button
                        className="absolute bottom-2 left-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center hover:bg-red-700 transition"
                        onClick={(e) => {
                            e.stopPropagation();
                            closeAccordion()
                        }}
                        title="Close"
                    >
                        <XCircleIcon className="h-4 w-4"/>
                    </button>
                )}
                <div
                    className="collapse-title text-xl font-medium p-4 flex flex-col md:flex-row justify-start gap-4 items-start"
                >
                    <p
                        className="text-gray-600 w-full md:w-1/3 font-bold truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={currentGame.formattedStartDate}
                    >
                        StartDate:{currentGame.formattedStartDate}
                    </p>
                    <p
                        className="text-gray-600 w-full md:w-1/3  truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={format(currentGame.extractionDate!, "dd/MM/yyyy")}
                    >
                        Extract Date: {format(currentGame.extractionDate!, "dd/MM/yyyy")}
                    </p>
                    <p
                        className="text-gray-600 w-full md:w-1/3  truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={`Revenue ${currentGame.revenue} `}
                    >
                        Revenue: {currentGame.revenue}
                    </p>
                    <SequenceDisplay numbers={currentGame.extractedNumbers}
                                     winningNumbers={currentGame.extractedNumbers}></SequenceDisplay>

                </div>
                {isOpen && (
                    <div className="w-full p-4 flex flex-col items-center justify-center gap-2">
                        {players.length > 0 ?
                            (players.map((p) =>
                                (<WinningPlayerDisplay winningNumbers={currentGame.extractedNumbers!} key={p.playerId}
                                                       playerInfo={p}/>))) : ("No available data")}
                        <Pagination currentPage={currentPagination.currentPage} onPageChange={handlePageChange}
                                    totalPages={Math.ceil(currentPagination.totalItems / currentPagination.currentPageItems)}></Pagination>
                    </div>
                )}

            </div>
        </div>
    )


}