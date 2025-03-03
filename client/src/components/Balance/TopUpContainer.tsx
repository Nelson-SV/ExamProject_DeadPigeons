import {TopUpValue, UploadFile} from "./index.ts";
import {useEffect, useState} from "react";
import {BalanceTopUpValue} from "/src/Api.ts";
import {useConfiguration} from "/src/hooks";


export const TopUpContainer = () => {
    const {getTopUpValues} = useConfiguration();
    const [topUpValues, setTopUpValues] = useState<BalanceTopUpValue[]>([]);
    const [error, setError] = useState<string>("");
    const [topUpValue, setTopUpValue] = useState(0);
    const [selectedButton, setSelectedButton] = useState("");


    useEffect(() => {
        const fetchData = async () => {
            try {
                const topUpData = await getTopUpValues();
                setTopUpValues(topUpData);

                const ticketData = await getTopUpValues();
                setTopUpValues(ticketData);
            } catch (err: any) {
                setError(err.message);
            }
        };
        fetchData();
    }, []);


    const getTopUpValue = (value: number) => {
        setTopUpValue(value);
    }
    const getSelectedValue = (buttonId:string) => {
        return selectedButton;
    }
    const setSelectedValue = (value: string) => {
        setSelectedButton(value);
    }


    return (
        <div
            className={"flex flex-col justify-start items-center  bg-background rounded-lg p-10 shadow-lg  sm:w1/2  md:w-2/4 lg:w-3/4 xl:w-2/4  mx-auto "}>
            <div className={"grid  grid-cols-2 grid-rows-3  gap-10 w-3/4 h-full "}>
                {topUpValues.map((value) => {
                    return (
                        <TopUpValue getSelectedButton={getSelectedValue} setSelectedButtonId={setSelectedValue}
                                    buttonId={value.topUpValue + ""} key={value.topUpValue} value={value.topUpValue}
                                    getValue={getTopUpValue}></TopUpValue>
                    )
                })}

            </div>
            <UploadFile topUpValue={topUpValue}></UploadFile>
        </div>


    )


}