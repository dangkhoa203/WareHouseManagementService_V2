import Container from "@mui/material/Container";
import {
    Checkbox,
    FormControl,
    Grid2,
    Input,
    InputAdornment,
    InputLabel, LinearProgress, Paper,
    TextField
} from "@mui/material";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import {Visibility, VisibilityOff} from "@mui/icons-material";
import {useEffect, useState} from "react";
import userInfo from "../../Type/userInfo.tsx";
import {Navigate, useOutletContext} from "react-router";
import {useUserInfo} from "../../State/User.ts";
interface loginInfo{
    userName: string;
    password: string;
    remember: boolean;
}
interface loginError{
    Username:string,
    Password:string,
    Global:string
}
export default function Login(){
    const userInfo = useUserInfo((state)=> state.user);
    const [loginInfo,setLoginInfo]=useState<loginInfo>({
        userName:"",
        password:"",
        remember:false,
    })
    const [error,setError]=useState<loginError>({
        Username:"",
        Password:"",
        Global:""
    })
    const [loading,setLoading]=useState<boolean>(false);
    const [showPassword, setShowPassword] = useState(false);
    const checkLoginInfo=()=>{
        const errorModel:loginError={
            Username:'',
            Password:'',
            Global:''
        }
        setError(errorModel)
        if(loginInfo.userName.length===0||loginInfo.password.length===0){
            setError({...errorModel,Global:"Chưa nhập thông tin đầy đủ!"})
            return false;
        }
        return true;
    }
    const handleClickShowPassword = () => setShowPassword((show) => !show);

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    const handleMouseUpPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };


    const handleUserNameChange=(e: React.ChangeEvent<HTMLInputElement>) => {
        setLoginInfo({...loginInfo,userName:e.target.value});
    }
    const handlePasswordChange=(e: React.ChangeEvent<HTMLInputElement>) => {
        setLoginInfo({...loginInfo,password:e.target.value});
    }
    const handleRememberChange=(e: React.ChangeEvent<HTMLInputElement>) => {
        setLoginInfo({...loginInfo,remember:e.target.checked});
    }

    const login= async ()=>{
        // if (checkLoginInfo()) {
        //     try {
        //         setLoading(true)
        //         const response = await fetch('https://localhost:7075/api/Account/Login', {
        //             method: 'POST',
        //             headers: {'Content-Type': 'application/json'},
        //             credentials: 'include',
        //             body: JSON.stringify(loginInfo)
        //         })
        //         if (!response.ok) {
        //             const content = await response.json();
        //             const errormessage:loginError={
        //                 Username:"",
        //                 Password:"",
        //                 Global:""
        //             }
        //             if( content.validateError!==null && !content.validateError.isValid) {
        //                 const list = content.validateError.errors
        //
        //                 list.forEach((element: any) => {
        //
        //                     if (element.propertyName === "UserName")
        //                         setError({...error, Username: element.errorMessage})
        //
        //                     if (element.propertyName === "Password"){
        //                         errormessage.Password=element.errorMessage
        //
        //                     }
        //
        //                 })
        //             }
        //             errormessage.Global=content.errorMessage
        //             setError(errormessage)
        //             console.log(error)
        //         }else{
        //             await getInfo()
        //         }
        //     } catch  {
        //         console.log("Error")
        //     } finally {
        //         setLoading(false)
        //     }
        // }
    }
    useEffect(()=>{
        document.title="Đăng nhập"
    },[])
    if(userInfo.isLoggedIn){
        return <Navigate to="/"/>
    }
    return (
        <Container maxWidth="lg" sx={{display:"flex",justifyContent:"center",paddingTop:"30px"}} >
            <Paper elevation={12}
                   property="div"
                     sx={{
                border:"3px solid orange",
                minHeight:"400px",
                width:"500px",
                paddingY:"10px",
                display:"flex",flexDirection:"column"}}  >
                <Typography fontSize={"3.5em"} textAlign={"center"}>
                    Đăng nhập
                </Typography>
                <Grid2 container spacing={2} paddingX="50px" paddingTop="20px" paddingBottom="20px" >
                    <Grid2 sx={{textAlign:'center'}} size={12}>
                        <TextField value={loginInfo.userName} onChange={handleUserNameChange}
                                   color="warning" sx={{fontSize:"1.5em",width:"100%"}}
                                   label="UserName" variant="standard"
                                   error={error.Username.length!==0}
                                   helperText={error.Username}
                        />
                    </Grid2>
                    <Grid2 sx={{textAlign:'center'}} size={12}>
                        <FormControl fullWidth variant="standard">
                            <InputLabel color="warning" sx={{color:error.Password.length!==0 ?"rgba(237, 108, 2)":""}}>Password</InputLabel>
                            <Input
                                color="warning"
                                value={loginInfo.password}
                                error={error.Password.length!==0}
                                onChange={handlePasswordChange}
                                type={showPassword ? 'text' : 'password'}
                                endAdornment={
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label={
                                                showPassword ? 'hide the password' : 'display the password'
                                            }
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                            onMouseUp={handleMouseUpPassword}
                                        >
                                            {!showPassword ? <VisibilityOff className="password-icon" /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                        <p style={{fontSize:"12px",margin:0,marginTop:"3px",color:"rgba(237, 108, 2)"}} >{error.Password}</p>
                    </Grid2>
                    <Grid2 sx={{textAlign:'end'}} size={12}>
                        <Checkbox value={loginInfo.remember} onChange={handleRememberChange} color={"warning"} sx={{marginLeft:0}}/>
                        <span
                            style={{color: loginInfo.remember ? "rgba(237, 108, 2)":" ",fontSize:"14px",transition:"0.3s all"}}>Nhớ đăng nhập</span>
                    </Grid2>
                </Grid2>

                    <p style={{fontSize:"1.3em",paddingBottom:"10px",textAlign:"center",margin:0,marginTop:"3px",color:"rgba(237, 108, 2)"}}>
                        {error.Global}
                    </p>

                {loading ?
                    <LinearProgress sx={{marginX:"20px",minHeight:"10px"}} color="warning" />
                    :
                    <Button sx={{marginX:"100px"}} color={"warning"}  onClick={()=>login()} variant="outlined">Đăng nhập</Button>
                }

            </Paper>
        </Container>
    )
}