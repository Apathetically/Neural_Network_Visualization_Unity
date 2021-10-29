import sys
import numpy as np
import torch
from torch import nn
from torch.autograd import Variable
from torchvision.datasets import CIFAR10
from utils import train
sys.path.append('..')

def data_tf(x):
    x = np.array(x, dtype='float32') / 255
    x = (x - 0.5) / 0.5
    x = x.transpose((2, 0, 1))
    x = torch.from_numpy(x)
    return x

train_set = CIFAR10('./data', train=True, transform=data_tf, download=True)
train_data = torch.utils.data.DataLoader(train_set, batch_size=256, shuffle=True)
test_set = CIFAR10('./data', train=False, transform=data_tf, download=True)
test_data = torch.utils.data.DataLoader(test_set, batch_size=256, shuffle=True)

class my_net(nn.Module):
    def __init__(self):
        super(my_net, self).__init__()
        self.stage1 = nn.Sequential(
            nn.Conv2d(3, 4, 3, padding=1), # 32x32
        )
        self.stage2 = nn.Sequential(
            nn.Conv2d(4, 4, 3, padding=1), # 32x32
        )
        self.stage3 = nn.Sequential(
            nn.BatchNorm2d(4),
            nn.ReLU(True),
            nn.MaxPool2d(2, 2), # 16x16
            nn.Conv2d(4, 16, 3, padding=1), # 16x16
        )
        self.stage4 = nn.Sequential(
            nn.Conv2d(16, 16, 3, padding=1), # 16x16
        )
        self.stage5 = nn.Sequential(
            nn.BatchNorm2d(16),
            nn.ReLU(True),
            nn.MaxPool2d(2, 2), # 8x8
        )
        self.classfy = nn.Linear(1024, 10)
    def forward(self, x):
        res = []
        x = self.stage1(x)
        res.append(x.clone()) # 4x32x32
        x = self.stage2(x)
        res.append(x.clone()) # 4x32x32
        x = self.stage3(x)
        res.append(x.clone()) # 16x16x16
        x = self.stage4(x)
        res.append(x.clone()) # 16x16x16
        x = self.stage5(x)
        x = x.view(x.shape[0], -1)
        res.append(x.resize(x.shape[0], 1, 32, 32))
        x = self.classfy(x)
        return x, res

net = my_net()
criterion = nn.CrossEntropyLoss()
optimizer = torch.optim.Adam(net.parameters(), 1e-3)

train(net, train_data, test_data, 100, optimizer, criterion)