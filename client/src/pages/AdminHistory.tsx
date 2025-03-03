import {SearchWindowComponent} from "/src/components/AdminComponents/GameHistory/Search.tsx";
import {GameHistory} from "/src/components/AdminComponents/GameHistory/GamesHistoryDisplay.tsx";

export const AdminHistory =()=>{

    return (
        <div className="flex  flex-grow  container w-screen mx-auto py-8 ">
              <GameHistory></GameHistory>
               <SearchWindowComponent></SearchWindowComponent>
        </div>



    )
}