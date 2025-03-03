import {useAtom, useAtomValue} from "jotai";
import {WinningSequenceAtom, WinningSequenceResponse} from "/src/atoms";
import React, {useEffect, useState} from "react";
import {http, showErrorToasts, StorageKeys, useStorageHandler} from "/src/helpers";
import {WinningPlayer, WinningPlayersDto, WinningPlayersRequestDto} from "/src/Api.ts";
import {AdminWiningTicketsPaginationAtom} from "/src/atoms/adminAtoms/AdminWiningTicketsPaginationAtom.ts";
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";
import {WinningPlayerDisplay} from "/src/components/AdminComponents/adminMainPage/WinningPlayerDisplay.tsx";
import Pagination from "/src/components/shared/Pagination.tsx";
import {Pagination as PagingType} from "/src/Api.ts";



export const WinningTicketsDisplay = () => {
    const processedWinningSequence = useAtomValue(WinningSequenceResponse);
    const storageHandler =  useStorageHandler;
    const winningSequence = useAtomValue(WinningSequenceAtom);
    const [currentPagination, setCurrentPagination] = useAtom(AdminWiningTicketsPaginationAtom);
    const [players, setPlayers] = useState<WinningPlayer[]>([]);


    useEffect(() => {
        if (processedWinningSequence.registered) {
            retrieveWinningPlayers(currentPagination);
        }
    }, [processedWinningSequence]);

    const retrieveWinningPlayers = (currentPagination: PagingType) => {
        if (!processedWinningSequence.registered) {
            toast.error("An error occurred while retrieving the game info ");
            return;
        }

        const gameInfo = storageHandler.getItem(StorageKeys.GAME_INFO);
        if (!gameInfo.guid) {
            toast.error("Game info missing or invalid in storage.Please retry");
            return;
        }

        const requestObject: WinningPlayersRequestDto = {
            gameId: {guid:storageHandler.getItem(StorageKeys.GAME_INFO).guid},
            winningSequence: winningSequence,
            pagination: currentPagination
        }
        http.api.gameHandlerGetWinningPlayers(requestObject)
            .then((r: AxiosResponse<WinningPlayersDto>) => {
                if (r.status === 200) {
                    setCurrentPagination(r.data.pagination!);
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

    const getCurrentPage = (currentPage: number) => {
        const updatedPagination = {
            ...currentPagination,
            currentPage: currentPage
        };
        setCurrentPagination(updatedPagination);
        retrieveWinningPlayers(updatedPagination);
    }

    return (
        <div className={"flex flex-col items-center justify-start w-full mt-5 "}>
            {players.length > 0 &&
                <>
                    {players.map((wp) => (
                        <WinningPlayerDisplay winningNumbers={[]} key={wp.playerId} playerInfo={wp}/>
                    ))}

                    <Pagination
                        totalPages={Math.ceil(currentPagination.totalItems / currentPagination.currentPageItems)}
                        currentPage={currentPagination.currentPage}
                        onPageChange={getCurrentPage}
                    />
                </>
            }

        </div>

    )
}