import {FC, JSX, ReactNode} from "react";
import './styles.scss';
import logo from "../../../public/logo.svg";
import {useNavigate} from "react-router-dom";

interface IHeaderProps {
  children?: JSX.Element;
}
export const Header: FC<IHeaderProps> = () => {
  const navigateTo = useNavigate();
  return (
    <div className='header'>
      <img className={'logo'} src={logo} onClick={() => navigateTo("/")} />
    </div>
  );
};
