import { useSelector, useDispatch } from 'react-redux';
import { RootState, AppDispatch } from '../store';
import { UserRole } from '../types';

export const useAuth = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { user, isAuthenticated, isLoading, error } = useSelector((state: RootState) => state.auth);

  const hasRole = (role: UserRole): boolean => {
    return user?.role === role;
  };

  const hasAnyRole = (roles: UserRole[]): boolean => {
    return user ? roles.includes(user.role) : false;
  };

  const isDealerUser = (): boolean => {
    return hasAnyRole(['DEALER_STAFF', 'DEALER_MANAGER']);
  };

  const isManufacturerUser = (): boolean => {
    return hasAnyRole(['EVM_STAFF', 'ADMIN']);
  };

  const canAccessDealerPortal = (): boolean => {
    return isDealerUser();
  };

  const canAccessManufacturerPortal = (): boolean => {
    return isManufacturerUser();
  };

  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    hasRole,
    hasAnyRole,
    isDealerUser,
    isManufacturerUser,
    canAccessDealerPortal,
    canAccessManufacturerPortal,
    dispatch,
  };
};
