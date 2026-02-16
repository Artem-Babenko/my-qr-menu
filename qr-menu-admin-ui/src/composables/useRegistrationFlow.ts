import { authApi } from '@/api/authApi';
import { getErrorMessage } from '@/consts/errorMessages';
import { useAuthStore } from '@/store/auth';
import { useUserStore } from '@/store/user';
import type { RegistrationFormFields, RegistrationReq } from '@/types/auth';
import { useToast } from '@/composables';
import { ROUTES } from '@/router';
import { useRouter } from 'vue-router';

interface RegistrationFlowOptions {
  phone: string;
  formFields: RegistrationFormFields;
  onSuccess?: (userId: number) => Promise<{ networkId: number | null }>;
}

export function useRegistrationFlow() {
  const toast = useToast();
  const authStore = useAuthStore();
  const router = useRouter();

  const register = async (
    options: RegistrationFlowOptions,
  ): Promise<boolean> => {
    const regReq: RegistrationReq = {
      phone: options.phone,
      name: options.formFields.name,
      surname: options.formFields.surname,
      email: options.formFields.email,
      password: options.formFields.password,
    };

    const regResp = await authApi.reg(regReq);
    if (!regResp.success || !regResp.data) {
      toast.error(getErrorMessage(regResp.errorCode));
      return false;
    }

    const loginResp = await authApi.login({
      phone: regReq.phone,
      password: regReq.password,
    });
    if (!loginResp.success || !loginResp.data) {
      toast.error(getErrorMessage(loginResp.errorCode));
      return false;
    }

    authStore.setToken(loginResp.data.token);

    const networkResult = options.onSuccess
      ? await options.onSuccess(regResp.data.userId)
      : { networkId: null };

    router.replace({
      name: networkResult.networkId ? ROUTES.dashboard : ROUTES.onboarding,
    });
    return true;
  };

  return { register };
}
