import {GameAnimation, SimpleRegistrationForm} from "/src/components/imports.ts";

export const RegisterPage=()=>{
    return (
        <div className={"flex flex-col justify-start items-center w-screen"}>
            <GameAnimation></GameAnimation>
            <SimpleRegistrationForm></SimpleRegistrationForm>
        </div>
    )
}