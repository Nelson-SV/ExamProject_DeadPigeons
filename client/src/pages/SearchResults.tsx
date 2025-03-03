import {useParams} from "react-router-dom";
import {GameDisplay} from "/src/components/AdminComponents/GameHistory/GameDisplay.tsx";
import {useEffect, useState} from "react";
import {CurrentGameDto, GameIdDto} from "/src/Api.ts";
import {Loading} from "/src/components/imports.ts";
import {http, showErrorToasts} from "/src/helpers";
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";

export const SearchResults = () => {
    const {gameId} = useParams();
    const [currentGame, setCurrentGame] = useState<CurrentGameDto | null>(null);
    const [isLoading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        retrieveSearchedGameDetails()
    }, []);

    const retrieveSearchedGameDetails = () => {
        const gameIdTemp: string = gameId ?? "";
        const gameDto: GameIdDto = {
            guid: gameIdTemp
        }
        http.api.gameHandlerGetGameById(gameDto)
            .then((r: AxiosResponse<CurrentGameDto>) => {
                if (r.status === 200) {
                    setCurrentGame(r.data);
                    setLoading(false);
                }
            }).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                showErrorToasts(errorData);
            } else {
                toast.error("Network error. Please try again later.");
            }
        }).finally(() => setLoading(false));
    }

    if (isLoading) {
        return <Loading/>
    }
console.log("On the page");
    return (
        <div className="mt-16 w-full flex flex-col justify-start items-center col p-4 mx-auto">
            <GameDisplay currentGame={currentGame!}></GameDisplay>
        </div>
    )


}