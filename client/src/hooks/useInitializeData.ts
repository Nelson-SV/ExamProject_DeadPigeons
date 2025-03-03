import {useAtom, useEffect, http, getUserInfoFromToken} from "./imports";
import {WeeklyBoards} from "/src/components/imports.ts";

export function useInitializeData() {

    const [weeklyBoards, setWeeklyBoards] = useAtom(WeeklyBoards);

    const userId: string = getUserInfoFromToken().userId;
    useEffect(() => {
        http.api.playGetAutomatedTickets({userId})
            .then((response) => {
                setWeeklyBoards(response.data);
            }).catch(e => {
            console.log(e)
        })
    }, [])
}