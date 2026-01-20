import Container from "@mui/material/Container";
import {
    Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle,
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
import {Navigate, useNavigate, useOutletContext} from "react-router";
import userInfo from "../../Type/userInfo.tsx";
import {useUserInfo} from "../../State/User.ts";


interface registerInfo{
    fullName:string;
    userName:string;
    email:string;
    password:string;
    confirmPassword:string;
}
interface registerError{
    FullName:string,
    Username:string,
    Password:string,
    ConfirmPassword:string,
    Email:string,
    Global:string
}
export default function Register(){
    const navigate = useNavigate();
    const userInfo = useUserInfo((state)=> state.user);
    const [registerInfo,setRegisterInfo] = useState<registerInfo>({
        fullName:"",
        password:'',
        confirmPassword:'',
        email:'',
        userName:''
    });
    const [error,setError]=useState<registerError>(
        {
            FullName:'',
            Username:'',
            Password:'',
            ConfirmPassword:'',
            Email:'',
            Global:''
        }
    );
    const [loading,setLoading] = useState<boolean>(false);
    const [success,setSuccess]=useState<boolean>(false)
    const [showPassword, setShowPassword] = useState(false);

    const handleClickShowPassword = () => setShowPassword((show) => !show);

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    const handleMouseUpPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };
    const checkRegisterInfo = () => {
        const errorModel:registerError={
            FullName:'',
            Username:'',
            Password:'',
            ConfirmPassword:'',
            Email:'',
            Global:''
        }
        setError(errorModel)

        let flag = true;

        if (registerInfo.fullName.length === 0 || registerInfo.userName.length === 0 || registerInfo.password.length === 0 || registerInfo.confirmPassword.length === 0 || registerInfo.email.length === 0) {
            setError({...errorModel,Global:"Chưa nhập thông tin đầy đủ!"})
            flag = false;
        }
        if (registerInfo.password.length < 3) {
            setError({...errorModel,Password:"Độ dài mật khẩu phải phải lớn hơn 3!"})
            flag = false;
        }
        if (registerInfo.password !== registerInfo.confirmPassword) {
            setError({...errorModel,ConfirmPassword:"Mật khẩu không giống với mật khẩu xác nhận!"})
            flag = false;
        }

        return flag;
    }
    const Register=async ()=>{
        if (checkRegisterInfo()) {
            try {
                setLoading(true)
                const response = await fetch('https://localhost:7075/api/Account/Register', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    credentials: 'include',
                    body: JSON.stringify(registerInfo)
                })
                if (!response.ok) {
                    const content = await response.json();
                    const errorMessage:registerError={
                        FullName:'',
                        Username:'',
                        Password:'',
                        ConfirmPassword:'',
                        Email:'',
                        Global:''
                    }
                    if( content.validateError!==null && !content.validateError.isValid) {
                        const list = content.validateError.errors

                        list.forEach((element: any) => {
                            if (element.propertyName === "FullName")
                                errorMessage.FullName = element.errorMessage;

                            if (element.propertyName === "UserName")
                                errorMessage.Username = element.errorMessage;

                            if (element.propertyName === "Email")
                                errorMessage.Email = element.errorMessage;

                            if (element.propertyName === "Password")
                                errorMessage.Password = element.errorMessage;

                            if (element.propertyName === "ConfirmPassword")
                                errorMessage.ConfirmPassword = element.errorMessage;
                        })
                    }
                    errorMessage.Global=content.errorMessage;
                    setError(errorMessage);
                }else
                setSuccess(true)
            } catch  {
                    console.log("Error")
            } finally {
                setLoading(false)
            }
        }
    }
    const handleFullName=(event: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterInfo({...registerInfo,fullName:event.target.value})
    }
    const handleUserName=(event: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterInfo({...registerInfo,userName:event.target.value})
    }
    const handlePassword=(event: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterInfo({...registerInfo,password:event.target.value})
    }
    const handleConfirmPassword=(event: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterInfo({...registerInfo,confirmPassword:event.target.value})
    }
    const handleEmail=(event: React.ChangeEvent<HTMLInputElement>) => {
        setRegisterInfo({...registerInfo,email:event.target.value})
    }
    useEffect(()=>{
        document.title="Đăng ký"
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
                display:"flex",flexDirection:"column"}}   >
                <Typography fontSize={"3.5em"} textAlign={"center"}>
                    Đăng ký
                </Typography>
                <Grid2 container spacing={2} paddingX="50px" paddingTop="20px" paddingBottom="25px" >
                    <Grid2 sx={{textAlign:'center'}} size={6}>
                        <TextField  color="warning" sx={{fontSize:"1.5em",width:"100%"}}
                                    label="Full Name" variant="standard"
                                    value={registerInfo.fullName}
                                    onChange={handleFullName}
                                    error={error.FullName.length!==0}
                                    helperText={error.FullName}
                        />
                    </Grid2>
                    <Grid2 sx={{textAlign:'center'}} size={6}>
                        <TextField color="warning" sx={{fontSize:"1.5em",width:"100%"}}
                                   label="User Name" variant="standard"
                                   value={registerInfo.userName}
                                   onChange={handleUserName}
                                   error={error.Username.length!==0}
                                   helperText={error.Username}
                        />
                    </Grid2>
                    <Grid2  size={6}>
                        <FormControl fullWidth variant="standard">
                            <InputLabel color="warning" sx={{color:error.Password.length!==0 ?"rgba(237, 108, 2)":""}}>Password</InputLabel>
                            <Input
                                error={error.Password.length!==0}
                                color="warning"
                                value={registerInfo.password}
                                onChange={handlePassword}
                                type={showPassword ? 'text' : 'password'}
                                endAdornment={
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label={
                                                showPassword ? 'hide the password' : 'display the password'
                                            }
                                            color={"warning"}
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                            onMouseUp={handleMouseUpPassword}
                                        >
                                            {!showPassword ? <VisibilityOff color={"action"} /> : <Visibility color={"action"}/>}
                                        </IconButton>
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                        <p style={{fontSize:"12px",margin:0,marginTop:"3px",color:"rgba(237, 108, 2)"}} >{error.Password}</p>
                    </Grid2>
                    <Grid2 size={6}>
                        <FormControl fullWidth variant="standard">
                            <InputLabel color="warning" sx={{color:error.ConfirmPassword.length!==0 ?"rgba(237, 108, 2)":""}}>Password</InputLabel>
                            <Input
                                error={error.ConfirmPassword.length!==0}
                                color="warning"
                                value={registerInfo.confirmPassword}
                                onChange={handleConfirmPassword}
                                type={showPassword ? 'text' : 'password'}
                                endAdornment={
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label={
                                                showPassword ? 'hide the password' : 'display the password'
                                            }
                                            color={"warning"}
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                            onMouseUp={handleMouseUpPassword}
                                        >
                                            {!showPassword ? <VisibilityOff color={"action"} /> : <Visibility color={"action"}/>}
                                        </IconButton>
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                        <p style={{fontSize:"12px",margin:0,marginTop:"3px",color:"rgba(237, 108, 2)"}}>{error.ConfirmPassword}</p>
                    </Grid2>
                    <Grid2 sx={{textAlign:'center'}} size={12}>
                        <TextField color="warning" sx={{fontSize:"1.5em",width:"100%"}}
                                   label="Email" variant="standard"
                                   value={registerInfo.email}
                                   onChange={handleEmail}
                                   error={error.Email.length!==0}
                                   helperText={error.Email}
                        />
                    </Grid2>
                </Grid2>
                {error.Global.length !==0 &&
                    <p style={{fontSize:"1.3em",paddingBottom:"10px",textAlign:"center",margin:0,marginTop:"3px",color:"rgba(237, 108, 2)"}}>
                        {error.Global}
                    </p>
                }
                {loading ?
                    <LinearProgress sx={{marginX:"20px",minHeight:"10px"}} color="warning" />
                    :
                    <Button onClick={()=>Register()} sx={{marginX:"100px"}} color="warning" variant="outlined">Đăng ký</Button>
                }

            </Paper>
            <Dialog
                open={success}
                fullWidth
                maxWidth="md"
                disableScrollLock={false}
            >
                <DialogTitle style={{textAlign:"center",fontSize:"2rem",borderTop:"10px solid #E78B48"}}>
                    {"ĐĂNG KÝ THÀNH CÔNG"}
                </DialogTitle>
                <DialogContent >
                    <DialogContentText id="alert-dialog-description">
                        Email xác nhận tài khoản sẽ được gửi đến email đăng kỳ!
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button sx={{width:{xs:"200px",sm:"200px",md:"150px",lg:"100px"}}} onClick={()=>navigate("/Login")}  autoFocus>
                        OK
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    )
}