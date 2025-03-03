import {TopUpContainer, TopUpInstructions} from "../components/imports.ts";

export const BalancePage = () => {
    return (
        <div className={"flex flex-col  flex-grow py-10 gap-10 h-full w 3/4  "}>
            <TopUpInstructions></TopUpInstructions>
            <TopUpContainer/>

        </div>
    )
}