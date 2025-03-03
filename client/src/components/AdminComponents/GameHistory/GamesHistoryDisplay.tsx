import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {RetrievedGamesAtom} from "/src/atoms";
import {GameDisplay} from "/src/components/AdminComponents/GameHistory/GameDisplay.tsx";
import {Pagination as PagingType, TupleOfListOfCurrentGameDtoAndPagination} from "/src/Api.ts";
import {http, showErrorToasts} from "/src/helpers"
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";
import {Loading} from "/src/components/imports.ts";
import Pagination from "/src/components/shared/Pagination.tsx";

export const GameHistory = () => {
    const [games, setGames] = useAtom(RetrievedGamesAtom);
    const [isLoading, setLoading] = useState<boolean>(true);
    const [currentPagination, setCurrentPagination] = useState<PagingType>({
        currentPageItems: 10,
        currentPage: 1,
        nextPage: 2,
        hasNext: false,
        totalItems: 10
    });
    useEffect(() => {
        retrieveGames(currentPagination);
    }, []);
    const handlePageChange= (currentPage:number)=>{
        const updatedPagination = {
            ...currentPagination,
            currentPage: currentPage
        };
        setCurrentPagination(updatedPagination);
        retrieveGames(updatedPagination);
    }

    const retrieveGames = (pagination:PagingType) => {
        http.api.gameHandlerGetGames(pagination)
            .then((r: AxiosResponse<TupleOfListOfCurrentGameDtoAndPagination>) => {
                if (r.status === 200) {
                    setGames(r.data.item1!);
                    setCurrentPagination(r.data.item2!)
                }
            }).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                showErrorToasts(errorData);
            } else {
                toast.error("Network error. Please try again later.");
            }
        }).finally(() => setLoading(false))

    }


    if (isLoading) {
        return <Loading></Loading>
    }
    return (
        <div className="mt-16 w-full flex flex-col justify-start items-center col p-4 mx-auto">
            {
                games.map((g) =>
                    (<GameDisplay  key={g.guid} currentGame={g}/>))
            }
            <Pagination currentPage={currentPagination.currentPage} onPageChange={handlePageChange} totalPages={Math.ceil(currentPagination.totalItems / currentPagination.currentPageItems)}></Pagination>
        </div>
    )


}




