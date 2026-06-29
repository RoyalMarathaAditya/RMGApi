import toast from 'react-hot-toast';

const defaultOptions = {
  duration: 4000,
};

export const toastService = {
  success: (message: string) => toast.success(message, defaultOptions),
  error: (message: string) => toast.error(message, { ...defaultOptions, duration: 5000 }),
  warning: (message: string) => toast(message, { ...defaultOptions, icon: '⚠️' }),
  info: (message: string) => toast(message, { ...defaultOptions, icon: 'ℹ️' }),
};
