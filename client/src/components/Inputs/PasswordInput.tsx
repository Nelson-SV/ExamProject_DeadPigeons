import { useState} from "react";

interface PasswordParams {
    getPassword: (password: string) => void
    value:string
    title:string
    placeholder:string
}

export const PasswordInput = ({getPassword, value,title,placeholder}: PasswordParams) => {
    const [inputState, setInputState] = useState({
        type: "password",
    })


    const showPassword = () => {
        let show = inputState.type === "password" ? "text" : "password";
        setInputState((prev) => (
            {
                ...prev,
                type: show
            })
        )
    }


    return (

        <div className={"grow w-full flex flex-col items-stretch mb-2"}>
            <label
                htmlFor={title}
                className="text-sm font-large text-gray-700"
            >
                <span>
                    {title}
                </span>
            </label>
            <div className="input input-bordered flex  items-center  gap-2">
                <svg
                    onClick={showPassword}
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 16 16"
                    fill="currentColor"
                    className="h-4 w-4 opacity-70"
                >
                    <path
                        fillRule="evenodd"
                        d="M14 6a4 4 0 0 1-4.899 3.899l-1.955 1.955a.5.5 0 0 1-.353.146H5v1.5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1-.5-.5v-2.293a.5.5 0 0 1 .146-.353l3.955-3.955A4 4 0 1 1 14 6Zm-4-2a.75.75 0 0 0 0 1.5.5.5 0 0 1 .5.5.75.75 0 0 0 1.5 0 2 2 0 0 0-2-2Z"
                        clipRule="evenodd"
                    />
                </svg>
                <input
                    id={title}
                    type={inputState.type}
                    onChange={(e) => getPassword(e.target.value)}
                    className="grow w-full  md:w-1/2 lg:w-1/3 xl:w-1/4 p-2 border rounded "
                    value={value}
                    placeholder={placeholder}
                />
            </div>
        </div>
    )
}