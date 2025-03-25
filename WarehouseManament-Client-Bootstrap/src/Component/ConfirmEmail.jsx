import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
export default function ConfirmEmail() {
    const navigate=useNavigate()
    const { username, code } = useParams()
    async function ConfirmAccount(username,code) {
        const response = await fetch(`https://localhost:7075/api/Account/ConfirmAccount/${username}/${code}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
        })
        if(response.ok){
            navigate("/")
        }
        else{
            navigate("/Error")
        }
    }
    useEffect(()=>{
        ConfirmAccount(username,code)
    },[])
    return (
        <>
        </>
    )
}